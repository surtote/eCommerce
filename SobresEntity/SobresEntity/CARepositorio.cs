using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SobresEntity
{
    public class CARepositorio
    {
        public static List<Clase> GetClases() {
            AlumnosDBContext alumnosDBContext = new AlumnosDBContext();
            return AlumnosDBContext.Clases.ToList();
                }
    }
}