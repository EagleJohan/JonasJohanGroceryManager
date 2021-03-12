using Microsoft.VisualStudio.TestTools.UnitTesting;
using JonasOchJohansMataffär;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using ProductManager;

namespace JonasOchJohansMataffär.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void BasicReadOffering()
        {
            string[] a1 = { "hej", "hejdå", "20", "viSes" };
            string[] a2 = { "Benny", "Erik", "30", "Olle" };
            string[] a3 = { "Spindel", "Iller", "34", "Hund" };
            List<Product> products = new List<Product>();

            List<string[]> file = new List<string[]> { a1, a2, a3 };
            MainWindow.ReadOfferings(file, products);

            Assert.AreEqual("hej", products[0].ArticleDescription);
        }

        [TestMethod()]
        public void ReadOfferingFromFile()
        {
            string inventoryPath = "TestInventory.txt";
            List<Product> products = new List<Product>();

            List<string[]> file = (File.ReadLines(inventoryPath).Select(a => a.Split(';')).ToList());
            MainWindow.ReadOfferings(file, products);

            Assert.AreEqual("Spindel", products[2].ArticleDescription);
        }

        [TestMethod()]
        public void WriteDiscountCodesToFile()
        {
            Discount myDiscount = new Discount();
            myDiscount.discountCodes = new Dictionary<string, decimal>();
            myDiscount.discountCodes.Add("brad10", 0.1M);
            myDiscount.discountCodes.Add("angelina20", 0.2M);

            myDiscount.UpdateCSVFile("TestDiscount.txt");

            string[] testFile = File.ReadAllLines("TestDiscount.txt");

            Assert.AreEqual(testFile[0], "brad10;0,1");
        }

        [TestMethod()]
        public void ReadDiscountCodesFromFile()
        {
            Discount myDiscount = new Discount();
            myDiscount.discountCodes = new Dictionary<string, decimal>();
            myDiscount.ReadDiscountCodes("TestDiscount.txt");
            Assert.AreEqual(0.1M, myDiscount.discountCodes["brad10"]);
           
        }

        [TestMethod()]
        public void ItemRemoveCSVFile()
        {
            ItemRemove myItemRemove = new ItemRemove();
            string[] a1 = { "hej", "hejdå", "20", "viSes" };
            string[] a2 = { "Benny", "Erik", "30", "Olle" };
            string[] a3 = { "Spindel", "Iller", "34", "Hund" };

            myItemRemove.file = new List<string[]> { a1, a2, a3 };

            myItemRemove.UpdateCSVFile("TestItemRemove.txt");

            string[] testFile = File.ReadAllLines("TestItemRemove.txt");
            Assert.AreEqual(3, testFile.Length);
        }
    }
}
