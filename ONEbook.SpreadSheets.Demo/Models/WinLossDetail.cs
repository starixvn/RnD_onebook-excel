namespace ONEbook.SpreadSheets.Demo.Models
{
    public class WinLossDetail
    {
        public WinLossDetail()
        {
        }

        public WinLossDetail(
            int custId,
            string custName,
            decimal grossComm,
            decimal netTurnover,
            decimal turnover,
            int numberOfTickets,
            NetWinLoss memberNetWinLoss,
            NetWinLoss agentNetWinLoss,
            NetWinLoss masterNetWinLoss,
            NetWinLoss superNetWinLoss)
        {
            CustId = custId;
            CustName = custName;
            GrossComm = grossComm;
            NetTurnover = netTurnover;
            Turnover = turnover;
            NumberOfTickets = numberOfTickets;
            MemberNetWinLoss = memberNetWinLoss;
            AgentNetWinLoss = agentNetWinLoss;
            MasterNetWinLoss = masterNetWinLoss;
            SuperNetWinLoss = superNetWinLoss;
        }

        public int CustId { get; }

        public string CustName { get; }

        public decimal Turnover { get; }

        public decimal NetTurnover { get; }

        public decimal GrossComm { get; }

        public int NumberOfTickets { get; }

        public NetWinLoss MemberNetWinLoss { get; }

        public NetWinLoss AgentNetWinLoss { get; }

        public NetWinLoss MasterNetWinLoss { get; }

        public NetWinLoss SuperNetWinLoss { get; }

        public decimal Company =>
            -(MemberNetWinLoss.Total
            + AgentNetWinLoss.Total
            + (MasterNetWinLoss?.Total ?? 0)
            + (SuperNetWinLoss?.Total ?? 0));
    }
}