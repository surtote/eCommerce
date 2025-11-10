using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GardetaSuarezAPI.Controllers
{
    public class CadenaController : ApiController
    {
        public IEnumerable<Cadenas> Get()
        {
            using (ExamenHotelesEntities examen = new ExamenHotelesEntities())
            {
                return examen.Cadenas.ToList();
            }
        }

    }
}
