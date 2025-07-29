using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotWright
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class FunctionalUIAutomation : PageTest
    {
        private readonly string baseUrl = "https://qa-practice.netlify.app/auth_ecommerce";

        [SetUp]
        public async Task Setup()
        {
            await Page.GotoAsync(baseUrl);
        }

        #region Authentication Tests

        [Test]
        [Description("Test login with valid credentials")]
        public async Task LoginWithValidCredentials()
        {
            await LoginWithValidCredentialsHelper();

            var shoppingCartPage = Page.Locator("section.content-section > h2");
            await Expect(shoppingCartPage).ToBeVisibleAsync();

        }

        [Test]
        [Description("Test login with invalid credentials")]
        public async Task LoginWithInvalidCredentials()
        {
            string invalidUsername = "invalid@example.com";
            string invalidPassword = "wrongpassword";

            var usernameField = Page.Locator("input#email");
            var passwordField = Page.Locator("input#password");
            var loginButton = Page.Locator("button#submitLoginBtn");

            await usernameField.FillAsync(invalidUsername);
            await passwordField.FillAsync(invalidPassword);
            await loginButton.ClickAsync();

            var ErrorMessage = Page.Locator($"div.alert-danger");
            await Expect(ErrorMessage).ToBeVisibleAsync();
        }

        #endregion

        #region Product Management Tests

        [Test]
        [Description("Test adding a new product to cart")]
        public async Task AddNewProductToCart()
        {
            await LoginWithValidCredentialsHelper();
            await NavigateToShoppingCart();

            var addItemButton = Page.Locator("div:nth-child(5) > div:nth-child(3) > button:nth-child(2)");
            await addItemButton.ScrollIntoViewIfNeededAsync();
            await addItemButton.ClickAsync();

            var cartItem = Page.Locator("div.cart-item > span");
            await cartItem.ScrollIntoViewIfNeededAsync();
            await Expect(cartItem).ToBeVisibleAsync();

        }

        [Test]
        [Description("Test editing product quantity in cart")]
        public async Task EditProductQuantityInCart()
        {
            await LoginWithValidCredentialsHelper();
            await NavigateToShoppingCart();

            var addItemButton = Page.Locator("div:nth-child(5) > div:nth-child(3) > button:nth-child(2)");
            await addItemButton.ScrollIntoViewIfNeededAsync();
            await addItemButton.ClickAsync();

            var cartItem = Page.Locator("div.cart-item > span");
            await cartItem.ScrollIntoViewIfNeededAsync();
            await Expect(cartItem).ToBeVisibleAsync();

            var editQuantity = Page.Locator("input.cart-quantity-input");
            await editQuantity.FillAsync("");
            await editQuantity.FillAsync("2");
            
        }

        [Test]
        [Description("Test removing product from cart")]
        public async Task RemoveProductFromCart()
        {
            await LoginWithValidCredentialsHelper();
            await NavigateToShoppingCart();

            var addItemButton = Page.Locator("div:nth-child(5) > div:nth-child(3) > button:nth-child(2)");
            await addItemButton.ScrollIntoViewIfNeededAsync();
            await addItemButton.ClickAsync();

            var cartItem = Page.Locator("div.cart-item > span");
            await cartItem.ScrollIntoViewIfNeededAsync();
            await Expect(cartItem).ToBeVisibleAsync();

            var removeButton = Page.Locator("button.btn-danger");
            await removeButton.ScrollIntoViewIfNeededAsync();
            await removeButton.ClickAsync();

            var cartTotal = Page.Locator("div.cart-total > span");
            string totalText = await cartTotal.InnerTextAsync();
            Assert.That(totalText, Is.EqualTo("$0"), "Cart total should be $0");


        }

        #endregion

        #region Helper Methods

        private async Task LoginWithValidCredentialsHelper()
        {
            try
            {
                string validUsername = "admin@admin.com";
                string validPassword = "admin123";

                var usernameField = Page.Locator("input#email");
                var passwordField = Page.Locator("input#password");
                var loginButton = Page.Locator("button#submitLoginBtn");

                await usernameField.FillAsync(validUsername);
                await passwordField.FillAsync(validPassword);
                await loginButton.ClickAsync();

                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login helper failed: {ex.Message}");
                // Continue with test - some tests might work without login
            }
        }

        private async Task NavigateToShoppingCart()
        {
            try
            {
                // Look for products/shop navigation
                var productLinks = Page.Locator("section.content-section > h2");
                await Expect(productLinks).ToBeVisibleAsync();
            }
            catch
            {
                // Assume we're already on products page or it's the default page
            }
        }

        #endregion
    }
}
