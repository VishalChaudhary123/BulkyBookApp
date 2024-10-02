using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public static class SD // Static Details
    {
        // Roles

        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        // Customer - Payment - Status

        public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";
        public const string StatusShipped = "Shipped";

        // Payment Status
        public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
		public const string PaymentStatusRejected = "Rejected";


        // Session - Key Name

        public const string SessionCart = "SessionShoppingCart";
		

	}
}
