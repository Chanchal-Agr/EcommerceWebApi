namespace OnlineShoppingE_CommerceApplication.Provider.CustomException
{
    public class StockUnavailableException : Exception
    {
        public override string Message
        {
            get
            {
                return "Stock not available";
            }
        }
       
    }
}
