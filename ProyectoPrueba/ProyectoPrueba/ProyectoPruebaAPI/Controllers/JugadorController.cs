using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProyectoPruebaAPI.Controllers
{
    public class JugadorController : ApiController
    {
        public IEnumerable<Jugador> Get()
        {
            using (LigaEntities liga = new LigaEntities())
            {
                return liga.Jugador.ToList();
            }
        }
       

    }
}
