using System;

namespace PrimeCalculator.TypeSafeEnums
{
    public class CalculationStatus
    {
        public static CalculationStatus Unknown = new CalculationStatus(0, "unknown");

        public static CalculationStatus InProgress = new CalculationStatus(1, "InProgress");

        public static CalculationStatus Done = new CalculationStatus(2, "Done");

        public static CalculationStatus Failed = new CalculationStatus(3, "Failed");

        public int StatusId { get; }

        public string StatusName { get; set; }

        private CalculationStatus(int statusId, string statusName) 
        {
            StatusId = statusId;
            StatusName = statusName;
        }

        public static CalculationStatus GetStatusFromId(int statusId)
        {
            return statusId switch
            {
                0 => Unknown,
                1 => InProgress,
                2 => Done,
                3 => Failed,
                _ => throw new ArgumentException($"The given argumetn is not avalid statis id: {statusId}"),
            };
        }
    }
}
