using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UCommerce.Presentation.Views.Orders;
using UCommerce.Presentation.Web.Controls;
using UCommerce.Presentation.Web.Pages;

namespace UCommerce.RazorStore.umbraco.OrderFlow
{
	public partial class InvoicePage : ViewEnabledControl<IEditOrderView>, ISection
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			InvoiceFrame.Src = "/emails/invoice?Orderguid=" + View.Order.OrderGuid;
		}

		public IList<ICommand> GetCommands()
		{
			throw new NotImplementedException();
		}

		public bool Show
		{
			get { return View.Order.OrderStatus.OrderStatusId == 4; }
			private set
			{
				
			}
		}
	}
}