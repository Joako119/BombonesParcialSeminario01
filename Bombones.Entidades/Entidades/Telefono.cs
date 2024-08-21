namespace Bombones.Entidades.Entidades
{
    public class Telefono
    {
        public int TelefonoId { get; set; }
        public string NroTelefono { get; set; } = null!;

        // Relaciones
        public List<ClienteTelefono> ClienteTelefonos { get; set; } = new List<ClienteTelefono>();
        public override string ToString()
        {
            return $"{NroTelefono}";
        }
    }
}
