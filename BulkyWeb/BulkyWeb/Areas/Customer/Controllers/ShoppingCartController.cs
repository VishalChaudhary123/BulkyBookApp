using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;


namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                // Remove from cart
                _unitOfWork.ShoppingCart.Delete(cartFromDb);

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            // Remove from cart
           
            _unitOfWork.ShoppingCart.Delete(cartFromDb);

            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser= _unitOfWork.ApplicationUser.Get(u => u.Id==userId);

            // Upating details
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;




            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]

        // Using ShoppingCartVM which is declared above the contructor and it binding the below properties from the view
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.ShoppingCartList  = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
				
		     ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;   
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
				// It means it is a regular customer account and we need to capture the payment

				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
				// It is a company account

				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}
            // Order Header
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            // Order Details

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                    
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                // It means it is a regular customer account and we need to capture the payment
                // Stripe Logic
                //var domain = "https://localhost:7007";
                var domain = Request.Scheme + "://" + Request.Host.Value + "/"; // get local host dynamically
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"/customer/shoppingcart/OrderConfirmation?orderId={ShoppingCartVM.OrderHeader.Id}",
					CancelUrl = domain + "/customer/shoppingcart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment"
				};

				foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,

                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

				try
				{
					var service = new SessionService();
					Session session = service.Create(options);

					_unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
					_unitOfWork.Save();

					Response.Headers.Add("Location", session.Url);
					return new StatusCodeResult(303);
				}
				catch (StripeException ex)
				{
					// Log or handle the exception here
					Console.WriteLine($"Stripe exception: {ex.Message}");
					return StatusCode(500, "Internal server error");
				}

			}

            return RedirectToAction(nameof(OrderConfirmation), new {orderId =  ShoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation (int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser");
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // This an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower() == "paid")
                {
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderId,SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
				}

                HttpContext.Session.Clear();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
            _unitOfWork.Save();
            return View(orderId);
        }
		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                // Default price
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
