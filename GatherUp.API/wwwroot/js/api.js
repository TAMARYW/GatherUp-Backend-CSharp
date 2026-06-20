/* ============================================================
   GatherUp — שכבת תקשורת משותפת עם ה-Web API
   ============================================================ */

const GatherUp = (() => {
  const TOKEN_KEY = 'gatherup_token';

  // NotificationType: 0-4
  const NOTIF_LABELS = {
    0: 'אישור הגעה של משתתף',
    1: 'תשלום שהתקבל',
    2: 'הצבעה בסקר',
    3: 'נפתח סקר חדש',
    4: 'פרטי האירוע השתנו',
  };
  function notifLabel(type) { return NOTIF_LABELS[type] ?? String(type); }

  function saveToken(token)  { localStorage.setItem(TOKEN_KEY, token); }
  function getToken()        { return localStorage.getItem(TOKEN_KEY); }
  function clearToken()      { localStorage.removeItem(TOKEN_KEY); }

  function base64UrlDecode(str) {
    str = str.replace(/-/g, '+').replace(/_/g, '/');
    while (str.length % 4) str += '=';
    return decodeURIComponent(
      atob(str).split('').map(c => '%' + c.charCodeAt(0).toString(16).padStart(2, '0')).join('')
    );
  }

  function decodeToken() {
    const token = getToken();
    if (!token) return null;
    try { return JSON.parse(base64UrlDecode(token.split('.')[1])); }
    catch { return null; }
  }

  const CLAIM_URIS = {
    nameIdentifier: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
    name:           'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
    email:          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
  };
  const SHORT_FALLBACKS = {
    nameIdentifier: ['nameid', 'sub'],
    name:           ['unique_name', 'name'],
    email:          ['email'],
  };
  function getClaim(payload, key) {
    if (!payload) return undefined;
    if (payload[CLAIM_URIS[key]] !== undefined) return payload[CLAIM_URIS[key]];
    for (const short of SHORT_FALLBACKS[key])
      if (payload[short] !== undefined) return payload[short];
    return undefined;
  }

  function getCurrentUser() {
    const payload = decodeToken();
    if (!payload) return null;
    const id    = getClaim(payload, 'nameIdentifier');
    const name  = getClaim(payload, 'name');
    const email = getClaim(payload, 'email');
    if (id === undefined) return null;
    return { id: parseInt(id, 10), name, email };
  }

  function isLoggedIn() {
    const payload = decodeToken();
    if (!payload) return false;
    if (payload.exp && Date.now() >= payload.exp * 1000) return false;
    return true;
  }

  function logout() {
    clearToken();
    // replace (לא href) - לא משאיר את עמוד ה"לפני התנתקות" בהיסטוריית הקדימה
    window.location.replace('login.html');
  }

  // הגנה מפני כפתור "אחורה" בדפדפן אחרי התנתקות: בדפדפנים מסוימים (Firefox/
  // Safari בעיקר) חזרה אחורה משחזרת עמוד מוגן ישירות מ-bfcache בלי להריץ
  // מחדש את ה-JS (ולכן בלי requireAuth) - ה-pageshow עם event.persisted=true
  // מזהה בדיוק את המקרה הזה, וכופה רענון מלא כדי שה-requireAuth ירוץ מחדש.
  window.addEventListener('pageshow', (event) => {
    if (event.persisted) {
      window.location.reload();
    }
  });

  function requireAuth() {
    if (!isLoggedIn()) { window.location.href = 'login.html'; return null; }
    return getCurrentUser();
  }

  async function apiFetch(path, { method = 'GET', body = null, isForm = false } = {}) {
    const headers = {};
    const token = getToken();
    if (token) headers['Authorization'] = 'Bearer ' + token;

    let fetchBody;
    if (body !== null) {
      if (isForm) { fetchBody = body; }
      else { headers['Content-Type'] = 'application/json'; fetchBody = JSON.stringify(body); }
    }

    let response;
    try {
      response = await fetch(path, { method, headers, body: fetchBody });
    } catch {
      throw new Error('לא ניתן להתחבר לשרת. ודאו שה-API רץ.');
    }

    if (response.status === 204) return null;

    const text = await response.text();
    let data = null;
    if (text) { try { data = JSON.parse(text); } catch { data = text; } }

    if (!response.ok) {
      const message = (data && data.error) ? data.error
        : response.status === 401 ? 'יש להתחבר מחדש.'
        : response.status === 403 ? 'אין לך הרשאה לבצע פעולה זו.'
        : response.status === 404 ? 'הפריט המבוקש לא נמצא.'
        : 'אירעה שגיאה (סטטוס ' + response.status + ').';
      const err = new Error(message);
      err.status = response.status;
      err.data   = data;
      throw err;
    }
    return data;
  }

  function showToast(message, type = 'success') {
    let stack = document.querySelector('.toast-stack');
    if (!stack) { stack = document.createElement('div'); stack.className = 'toast-stack'; document.body.appendChild(stack); }
    const toast = document.createElement('div');
    toast.className = 'toast toast-' + type;
    toast.textContent = message;
    stack.appendChild(toast);
    setTimeout(() => toast.remove(), 4200);
  }

  function formatDate(value) {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleDateString('he-IL', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  function formatDateTime(value) {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleDateString('he-IL', { year: 'numeric', month: 'short', day: 'numeric' })
      + ' · ' + d.toLocaleTimeString('he-IL', { hour: '2-digit', minute: '2-digit' });
  }

  function escapeHtml(str) {
    return String(str ?? '').replace(/[&<>"']/g, c =>
      ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  }

  function renderNav(activeKey) {
    const slot = document.getElementById('app-nav-slot');
    if (!slot) return;
    const user = getCurrentUser();
    const links = [
      { key: 'dashboard', href: 'dashboard.html', label: 'האירועים שלי' },
      { key: 'profile',   href: 'profile.html',   label: 'הפרופיל שלי' },
    ];
    slot.innerHTML = `
      <div class="app-nav">
        <a class="brand" href="dashboard.html"><span class="dot"></span> GatherUp</a>
        <nav>
          ${links.map(l => `<a href="${l.href}" ${l.key === activeKey ? 'style="color:var(--primary-dark);font-weight:700;"' : ''}>${l.label}</a>`).join('')}
          ${user ? `<span class="user-pill">👤 ${escapeHtml(user.name || 'משתמש')}</span>` : ''}
          <button class="linklike" id="nav-logout-btn">התנתקות</button>
        </nav>
      </div>`;
    document.getElementById('nav-logout-btn').addEventListener('click', logout);
  }

  return {
    saveToken, getToken, clearToken, decodeToken, getCurrentUser, isLoggedIn,
    logout, requireAuth, apiFetch, showToast, formatDate, formatDateTime,
    escapeHtml, notifLabel, renderNav,
  };
})();