using GatherUp.Core.DO;

namespace GatherUp.Core.Interfaces;

public interface IReceiptRepository
{
    ReceiptDetails? GetById(string receiptNumber);
    
    IEnumerable<ReceiptDetails> GetAll();

    void AddReceipt(ReceiptDetails receipt, string currentSourceFilePath);
}