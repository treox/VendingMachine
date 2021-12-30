using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("VendingMachine.Tests")]

namespace VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            VendingMachine vend = new VendingMachine();
            bool running = true;
            bool isMainMenu;

            do
            {
                Console.Clear();
                Console.WriteLine("Välkommen! Gör ett val i menyn [a] - [c]: ");
                Console.WriteLine("[a] Köp en produkt");
                Console.WriteLine("[b] Sätt in pengar");
                Console.WriteLine("[c] Avsluta transaktion");

                Console.WriteLine();
                Console.WriteLine("Följande produkter finns: ");
                Console.WriteLine("_______________");
                isMainMenu = true;
                string showAllMessage = vend.ShowAll(isMainMenu);

                Console.WriteLine(showAllMessage);
                Console.WriteLine("_______________");

                Console.WriteLine();
                Console.WriteLine("Pengar du har att hadla för: " + VendingMachine.moneyPoolSum + "kr");

                ConsoleKeyInfo menuChoice = Console.ReadKey();

                switch(menuChoice.Key)
                {
                    case ConsoleKey.A:
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Följande produkter finns: ");
                        Console.WriteLine("_______________");
                        isMainMenu = false;
                        showAllMessage = vend.ShowAll(isMainMenu);

                        Console.WriteLine(showAllMessage);
                        Console.WriteLine("_______________");

                        bool isNotSuccessOptA = true;
                        do
                        {
                            try
                            {
                                Console.WriteLine("Pengar du har att hadla för: " + VendingMachine.moneyPoolSum + "kr");
                                Console.WriteLine($"Välj en produkt [11] - [{VendingMachine.productsCollection.Count + 10}]: ");
                                int.TryParse(Console.ReadLine(), out int productIndex);

                                string msgHowToUseItem = vend.Purchase(productIndex, VendingMachine.moneyPoolSum);
                                Console.WriteLine(msgHowToUseItem);
                                isNotSuccessOptA = false;
                            }
                            catch
                            {
                                Console.WriteLine("Fel: Ogiltig inmatning!");
                            }
                        } while (isNotSuccessOptA);
                        break;

                    case ConsoleKey.B:
                        Console.Clear();
                        Console.WriteLine("Hur mycket vill du sätta in?");

                        int denomNumber = 1;
                        foreach(int den in VendingMachine.moneyDenominations)
                        {
                            Console.WriteLine($"[{denomNumber}] {den}kr");
                            denomNumber++;
                        }

                        bool isNotSuccessOptB = true;
                        do
                        {
                            try
                            {
                                Console.WriteLine("Välj typ av valör genom att välja nummer [1] - [8]: ");
                                int.TryParse(Console.ReadLine(), out int denominationIndex);

                                Console.WriteLine($"Välj hur många {VendingMachine.moneyDenominations[denominationIndex - 1]}kr du vill sätta in: ");
                                int.TryParse(Console.ReadLine(), out int times);

                                int totalFunds = vend.InsertMoney(denominationIndex - 1, times, VendingMachine.moneyPoolSum);
                                Console.WriteLine("Du har nu " + totalFunds + "kr att handla för!");
                                isNotSuccessOptB = false;
                            }
                            catch
                            {
                                Console.WriteLine("Fel: Ogiltig inmatning!");
                            }
                        } while(isNotSuccessOptB);
                        break;

                    case ConsoleKey.C:
                        Console.Clear();
                        Console.WriteLine();
                        string endTransactionMessage = vend.EndTransaction(VendingMachine.moneyPool);
                        Console.WriteLine("Tillbaka: " + endTransactionMessage);

                        Console.WriteLine();
                        Console.WriteLine("Transaktionen avslutas. Välkommen åter!");
                        Console.WriteLine("Tryck på valfri tangent för att avsluta...");
                        running = false;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("Gör ett val genom att trycka på [a] - [c]!");
                        break;
                }
                Console.ReadKey();
            } while (running);
        }
    }

    public class VendingMachine : IVending
    {
        internal static IList<int> moneyDenominations = Array.AsReadOnly( new int[8] { 1, 5, 10, 20, 50, 100, 500, 1000 } );
        internal static int[] moneyPool = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        internal static int moneyPoolSum = 0;
        internal static List<Product> productsCollection = new List<Product>()
        {
            new Beverage( 10, "Kaffe", "Drick ditt kaffe!"),
            new Beverage( 15, "CocaCola Zero", "Drick din CocaCola!"),
            new Beverage( 15, "Fanta Light", "Drick din Fanta!"),
            new Pastry( 25, "Arraksboll", "Ät arraksbollen!"),
            new Pastry( 30, "Dammsugare", "Ät dammsugaren!"),
            new Snack( 12, "Snickers", "Ät Snickers!"),
            new Snack( 12, "Mars", "Ät Mars!"),
            new Snack( 20, "Chips", "Ät dina chips!"),
            new Snack( 25, "Jordnötter", "Ät jordnötterna!")
        };

        public string Purchase(int itemNumber, int currentFunds)
        {
            string purchasedItemMsg = "";

            if (itemNumber >= 11 && itemNumber <= productsCollection.Count + 10)
            {
                int productsCollectionIndex = itemNumber - 11;

                int price = productsCollection[productsCollectionIndex].Price;

                if(currentFunds >= price)
                {
                    int change = currentFunds - price;
                    moneyPoolSum = change;

                    for(int d = moneyDenominations.Count - 1; d >= 0; d--)
                    {
                        int numberOfDenominations = change / moneyDenominations[d];
                        change = change % moneyDenominations[d];
                        
                        moneyPool[d] = numberOfDenominations;
                    }

                    purchasedItemMsg = productsCollection[productsCollectionIndex].Use();
                }
                else
                {
                    purchasedItemMsg = "Du har inte tillräckligt med pengar för denna produkt. Vänligen sätt in pengar!";
                }
            }
            else
            {
                throw new Exception();
            }

            return purchasedItemMsg;
        }

        public string ShowAll(bool mainMenuIsActive)
        {
            StringBuilder sbCollection = new StringBuilder();

            string returnedCollection = "";

            int itemNumber = 1;
            foreach (Product item in productsCollection)
            {
                string returnedPriceAndInfo = item.Examine();

                if (mainMenuIsActive)
                {
                    sbCollection.Append($"- {returnedPriceAndInfo}kr \n");
                }
                else
                {
                    sbCollection.Append($"[{itemNumber + 10}] {returnedPriceAndInfo}kr \n");
                }
                itemNumber++;
            }

            returnedCollection = sbCollection.ToString();

            return returnedCollection;
        }

        public int InsertMoney(int denIndex, int times, int currentFunds)
        {
            if(denIndex >= 0 && denIndex <= 7 && times > 0)
            {
                moneyPool[denIndex] += times;
                currentFunds += moneyDenominations[denIndex] * times;
                moneyPoolSum = currentFunds;
            }
            else
            {
                throw new Exception();
            }

            return currentFunds;
        }

        public string EndTransaction(int[] moneyPool)
        {
            StringBuilder sbChange = new StringBuilder();
            sbChange.Append(" \n");

            int denomIndex = 0;
            foreach(int change in moneyPool)
            {
                int amountOfChangePerDenominator = change;
                sbChange.Append(amountOfChangePerDenominator + " st " + moneyDenominations[denomIndex] + "kr \n");
                denomIndex++;
            }

            sbChange.Append("Totalt: " + moneyPoolSum + "kr \n");
            return sbChange.ToString();
        }
    }

    public interface IVending
    {
        public string Purchase(int itemNumber, int currentFunds);
        public string ShowAll(bool mainMenuIsActive);
        public int InsertMoney(int denIndex, int times, int currentFunds);
        public string EndTransaction(int[] moneyPool);
    }

    public abstract class Product
    {
        public int Price { get; set; }
        public string Info { get; set; }
        public string Message { get; set; }

        public Product(int price, string info, string message)
        {
            this.Price = price;
            this.Info = info;
            this.Message = message;
        }

        public abstract string Examine();
        public abstract string Use();
    }

    public class Beverage : Product
    {
        public Beverage(int price, string info, string message) : base(price, info, message)
        {

        }

        public override string Examine()
        {
            return "Dricka: " + this.Info + ": " + this.Price;
        }

        public override string Use()
        {
            return "Tack för att du handlade dricka: " + this.Message;
        }
    }

    public class Pastry : Product
    {
        public Pastry(int price, string info, string message) : base(price, info, message)
        {

        }

        public override string Examine()
        {
            return "Bakelse: " + this.Info + ": " + this.Price;
        }

        public override string Use()
        {
            return "Tack för att du handlade bakelse: " + this.Message;
        }
    }

    public class Snack : Product
    {
        public Snack(int price, string info, string message) : base(price, info, message)
        {

        }

        public override string Examine()
        {
            return "Snacks: " + this.Info + ": " + this.Price;
        }

        public override string Use()
        {
            return "Tack för att du handlade snacks: " + this.Message;
        }
    }
}
