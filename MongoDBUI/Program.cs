using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using DataAccessLibrary.Models;
using DataAccessLibray;
using Microsoft.Extensions.Configuration;
using SharpCompress.Readers;

namespace MongoDBUI
{
    class Program
    {
        private static MongoDBDataAccess db;
        private static readonly string tableName = "Contacts";
        static void Main(string[] args)
        {
            db = new MongoDBDataAccess("MongoContactsDB", GetConnectionString());

            ContactModel user = new ContactModel
            {
                FirstName = "Ipek",
                LastName = "Utlu"
            };
            user.EmailAddresses.Add(new EmailAddressModel{EmailAddress = "utluipekt@gmail.com"});
            user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "utlumurat@hotmail.com" });
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "5339293751" });
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "None" });

            //CreateContact(user);

            //GetAllContacts();

            //GetContactById("fe361a90-8094-49a5-a900-6567904bef31");

            //UpdateContactsFirstName("Murti", "fe361a90-8094-49a5-a900-6567904bef31");

            //RemovePhoneNumberFromUser("None", "5d066211-3340-491b-8b02-9365838d01f2");

            //RemoveUser("fe361a90-8094-49a5-a900-6567904bef31")

            Console.WriteLine("Done Processing MongoDB");
            Console.ReadLine();
        }

        private static void RemoveUser(string id)
        {
            Guid guid = new Guid(id);
            db.DeleteRecord<ContactModel>(tableName,guid);
        }
        private static void RemovePhoneNumberFromUser(string phoneNumber, string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);
            contact.PhoneNumbers = contact.PhoneNumbers.Where(x => x.PhoneNumber != phoneNumber).ToList();
            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static void UpdateContactsFirstName(string firstName, string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);
            contact.FirstName = firstName;
            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static void GetContactById(string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            Console.WriteLine($"{contact.Id}: {contact.FirstName} {contact.LastName}");
        }

        private static void GetAllContacts()
        {
            var contacts = db.LoadRecords<ContactModel>(tableName);
            foreach (var contact in contacts)
            {
                Console.WriteLine($"{contact.Id}: {contact.FirstName} {contact.LastName}");
            }
        }
        private static void CreateContact(ContactModel contact)
        {
            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static string GetConnectionString(string connectionStringName = "Default")
        {
            string output = "";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            output = config.GetConnectionString(connectionStringName);
            return output;
        }
    }
}
