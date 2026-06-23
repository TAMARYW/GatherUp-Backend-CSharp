using System;
using System.IO;
using System.Xml.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Infrastructure.XML;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Repositories;

public class ReceiptRepository : XmlRepository<ReceiptDetails>, IReceiptRepository
{
    private readonly string _storageDirectory;

    public ReceiptRepository() : base(Path.Combine("..", "XML", "ReceiptDetails.xml"), useSerializer: false)
    {
        _storageDirectory = Path.Combine("..", "Receipts");
    }

    public void AddReceipt(ReceiptDetails receipt, string currentSourceFilePath)
    {
        if (receipt == null) throw new ArgumentNullException(nameof(receipt));
        if (string.IsNullOrWhiteSpace(currentSourceFilePath)) throw new ArgumentException("Source file path cannot be empty.");

        XDocument doc = XMLDocManager.LoadDocument(_filePath, "Receipts");

        if (XMLDocManager.GetElementById(doc, "Receipt", receipt.ReceiptNumber) != null)
        {
            throw new BusinessValidationException($"קבלה עם מספר {receipt.ReceiptNumber} כבר קיימת במערכת.");
        }

        if (!Directory.Exists(_storageDirectory)) Directory.CreateDirectory(_storageDirectory);

        string fileExtension = Path.GetExtension(currentSourceFilePath);
        string newFileName = $"{receipt.ReceiptNumber}{fileExtension}";
        string destinationPath = Path.Combine(_storageDirectory, newFileName);

        if (File.Exists(destinationPath))
        {
            throw new BusinessValidationException($"קובץ עבור קבלה {receipt.ReceiptNumber} כבר קיים במערכת.");
        }

        File.Copy(currentSourceFilePath, destinationPath, overwrite: false);

        XElement newReceiptElement = new XElement("Receipt",
            new XAttribute("Id", receipt.ReceiptNumber),
            new XElement("Amount", receipt.Amount),
            new XElement("Date", receipt.Date.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement("SavedFilePath", destinationPath)
        );

        if (doc.Root != null)
        {
            doc.Root.Add(newReceiptElement);
            XMLDocManager.SaveDocument(_filePath, doc);
        }
    }

    public override ReceiptDetails? GetById(string receiptNumber)
    {
        XDocument doc = XMLDocManager.LoadDocument(_filePath, "Receipts");
        XElement? element = XMLDocManager.GetElementById(doc, "Receipt", receiptNumber);

        if (element == null) return null;

        string number = element.Attribute("Id")?.Value ?? receiptNumber;
        decimal amount = decimal.Parse(element.Element("Amount")?.Value ?? "0");
        DateTime date = DateTime.Parse(element.Element("Date")?.Value ?? DateTime.Now.ToString());

        return new ReceiptDetails(number, amount, date);
    }

    public override void Add(ReceiptDetails entity) => throw new NotSupportedException("Use AddReceipt(receipt, path) instead.");

    public override void Update(ReceiptDetails entity) => throw new ReceiptLockedException("לא ניתן לערוך קבלה פיננסית קיימת - קבלות נעולות לאחר יצירתן.");
    public override void Delete(int id) => throw new ReceiptLockedException("לא ניתן למחוק קבלה פיננסית - קבלות נעולות לאחר יצירתן.");
    public override ReceiptDetails? GetById(int id) => throw new NotSupportedException("Use GetById(string) instead.");
}
