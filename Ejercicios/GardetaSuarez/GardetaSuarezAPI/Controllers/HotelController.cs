using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GardetaSuarezAPI.Controllers
{
    public class HotelController : ApiController
    {
        public IEnumerable<Hoteles> Get()
        {
            using (ExamenHotelesEntities examen = new ExamenHotelesEntities()) {

                return examen.Hoteles.ToList();
            }
        }

    }
}
