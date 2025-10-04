using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class RefreshTokenRequest
    {
        public string refreshToken { get; set; }
        public string oldToken { get; set; }
    }
}
