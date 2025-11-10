using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace SobresEntity
{
    public class AlumnosDBContext : DbContext
    {
        public static DbSet<Clase> Clases { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
    }
}