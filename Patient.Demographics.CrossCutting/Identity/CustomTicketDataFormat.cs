using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class CustomTicketDataFormat: TicketDataFormat 
    {
        public CustomTicketDataFormat(IRefreshtokenDataProtector protector) : base(protector as IDataProtector)
        {
        }
    }
}
