namespace MES_ProcessSvc.Model
{
    public class ProdOrder
    {
        public string? OrderNumber { get; set; }

        public string? Material { get; set; }

        public string? Uom { get; set; }

        public double Quantity { get; set; }

        public string Location { get; set; }

        public string Status { get; set; } = "Scheduled";
    }
}
