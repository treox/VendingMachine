using System;
using Xunit;

namespace VendingMachine.Tests
{
    public class VendingMachineShould
    {
        [Fact]
        public void NotBeAbleToManipulateMoneyDenominations()
        {
            Assert.Throws<NotSupportedException>(() => VendingMachine.moneyDenominations[0] = 2);
        }
           
        [Fact]
        public void EnablePurchasingAProduct()
        {
            VendingMachine sut = new VendingMachine();
            string expectedMessage = "Drick ditt kaffe!";
            int testItemNumber = 11;
            int testFunds = 20;

            string actualMessage = sut.Purchase(testItemNumber, testFunds);

            Assert.EndsWith(expectedMessage, actualMessage);
        }

        [Fact]
        public void DeclinePurchasingAProductIfDonNotHaveEnoughFunds()
        {
            VendingMachine sut = new VendingMachine();
            string expectedMessage = "Vänligen sätt in pengar!";
            int testItemNumber = 11;
            int testFunds = 9;

            string actualMessage = sut.Purchase(testItemNumber, testFunds);

            Assert.EndsWith(expectedMessage, actualMessage);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public void ThrowAnExceptionEnteringWrongPurchaseOption(int testItemNumber)
        {
            VendingMachine sut = new VendingMachine();
            int testFunds = 20;

            Assert.Throws<Exception>(() => sut.Purchase(testItemNumber, testFunds));
        }

        [Fact]
        public void ShowAllProducts()
        {
            VendingMachine sut = new VendingMachine();
            string expectedMessageStartsWith = "[19] Snacks: Jordnötter: 25kr \n";
            bool menuIsActive = false;

            string actualMessage = sut.ShowAll(menuIsActive);

            Assert.EndsWith(expectedMessageStartsWith, actualMessage);
        }
        
        [Fact]
        public void EnableAddingMoneyToThePool()
        {
            int testIndex = 0;
            int testTimes = 2;
            int testFunds = 20;
            int expectedFunds = 22;
            VendingMachine sut = new VendingMachine();

            int actualFunds = sut.InsertMoney(testIndex, testTimes, testFunds);

            Assert.Equal(expectedFunds, actualFunds);
        }

        [Theory]
        [InlineData(9, 1)]
        [InlineData(-1, 1)]
        [InlineData(8, 0)]
        public void ThrowAnExceptionEnteringWrongMoneyInput(int testIndex, int testTimes)
        {
            VendingMachine sut = new VendingMachine();
            int testFunds = 20;

            Assert.Throws<Exception>(() => sut.InsertMoney(testIndex, testTimes, testFunds));
        }

        [Fact]
        public void GiveChangeWhenEndingATransaction()
        {
            VendingMachine sut = new VendingMachine();
            string expectedMessage = "1 st 10kr";
            int[] testMoneyPool = new int[8] { 1, 0, 1, 0, 0, 0, 0, 0 };

            string actualMessage = sut.EndTransaction(testMoneyPool);

            Assert.Contains(expectedMessage, actualMessage);
        }

        [Fact]
        public void ProvideInformationAboutAProduct()
        {
            Product sut = new Beverage(15, "test info", "test message");
            string expectedPriceAndInfo = "Dricka: test info: 15";

            string actualPriceAndInfo = sut.Examine();

            Assert.Equal(expectedPriceAndInfo, actualPriceAndInfo);
        }

        [Fact]
        public void OutputAMessageHowToUseAProduct()
        {
            Product sut = new Beverage(15, "test info", "test message");
            string expectedUseMessage = "Tack för att du handlade dricka: test message";

            string actualUseMessage = sut.Use();

            Assert.Equal(expectedUseMessage, actualUseMessage);
        }
    }
}
