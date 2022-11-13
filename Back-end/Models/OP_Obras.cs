using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoGobMx.Models
{
    public class OP_Obras
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObraId { get; set; }
        public string NombreObra { get; set; }
        public float Latitud { get; set; }
        public float Longitud { get; set; }
        public string Descripcion { get; set; }
        public int? EstatusObraId { get; set; }
        public bool Archivado { get; set; }

        [ForeignKey("EstatusObraId")]
        public OP_EstatusObras? OP_EstatusObras { get; set; }

    }
}
