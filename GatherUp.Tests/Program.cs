using System;
using System.Collections.Generic;
using System.IO;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Repositories;

namespace GatherUp.TestProject;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Starting GatherUp Infrastructure Test ===");

        string xmlDirectory = "XML";
        string receiptsDirectory = "Receipts";

        if (!Directory.Exists(xmlDirectory)) Directory.CreateDirectory(xmlDirectory);
        if (!Directory.Exists(receiptsDirectory)) Directory.CreateDirectory(receiptsDirectory);

        RunWithXmlRepository();

        Console.WriteLine("\n=== Test Finished. Check 'XML' and 'Receipts' folders! ===");
        Console.ReadLine();
    }

    static void RunWithXmlRepository()
    {
        Console.WriteLine("\n--- Running with XML File Repository ---");

        IRepository<Participant> participantRepo = new XmlRepository<Participant>(Path.Combine("XML", "Participant.xml"), useSerializer: true);
        if (!participantRepo.GetAll().GetEnumerator().MoveNext())
        {
            Console.WriteLine("Initializing default data into XML...");
            participantRepo.Add(new Participant { Id = 1, Name = "Tamar" });
            participantRepo.Add(new Participant { Id = 2, Name = "Avital" });
        }

        Console.WriteLine("\nAdding 3 new participants...");
        participantRepo.Add(new Participant { Name = "Gili" });
        participantRepo.Add(new Participant { Name = "Noam" });
        participantRepo.Add(new Participant { Name = "Michal" });

        Console.WriteLine("\nCurrent Participants List from XML:");
        foreach (var p in participantRepo.GetAll())
        {
            Console.WriteLine($"- ID: {p.Id}, Name: {p.Name}");
        }

        Console.WriteLine("\n--- Testing Receipt Upload & XML Logging ---");
        ReceiptRepository receiptRepo = new ReceiptRepository();

        string fakeUserSourceFile = "invoice.pdf";
        if (!File.Exists(fakeUserSourceFile))
        {
            File.WriteAllText(fakeUserSourceFile, "Dummy PDF Content for Receipt Test.");
        }

        string uniqueReceiptNumber = "REC-" + DateTime.Now.Ticks.ToString().Substring(10);
        ReceiptDetails newReceipt = new ReceiptDetails(uniqueReceiptNumber, 450.50m, DateTime.Now);

        try
        {
            Console.WriteLine($"Uploading receipt {uniqueReceiptNumber} to 'Receipts' folder...");

            receiptRepo.AddReceipt(newReceipt, fakeUserSourceFile);

            Console.WriteLine("Receipt uploaded and object added to XML successfully!");

            ReceiptDetails? retrievedReceipt = receiptRepo.GetById(uniqueReceiptNumber); if (retrievedReceipt != null)
            {
                Console.WriteLine($"Retrieved Receipt from XML -> Number: {retrievedReceipt.ReceiptNumber}, Amount: {retrievedReceipt.Amount}₪");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Receipt upload failed: {ex.Message}");
        }
    }
}