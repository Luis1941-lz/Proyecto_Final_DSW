namespace ProyectoApi_DSW.Models
{
    public class Carrito
    {
       
        public int IdCarrito { get; set; }
     
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }  

        public int IdLibro { get; set; }
        public Libro? Libro { get; set; }  

        public int Cantidad { get; set; } = 1;  
    }
}
