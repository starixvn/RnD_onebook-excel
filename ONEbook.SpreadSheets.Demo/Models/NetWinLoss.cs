namespace ONEbook.SpreadSheets.Demo.Models
{
    public class NetWinLoss
    {
        public NetWinLoss(decimal winloss, decimal comm)
        {
            WinLoss = winloss;
            Comm = comm;
        }

        public decimal WinLoss { get; }

        public decimal Comm { get; }

        public decimal Total => WinLoss + Comm;
    }
}