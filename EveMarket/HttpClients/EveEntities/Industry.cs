namespace EveMarket.HttpClients.EveEntities
{
    public class Industry
    {
        public record Job (int ActivityId, long BlueprintId, long BlueprintLocationId, int BlueprintTypeId, int? CompletedCharacterId, DateTime CompletedDate,
            double Cost, int Duration, DateTime EndDate, long FacilityId, int InstallerId, int JobId, int LicensedRuns, long OutputLocationId, DateTime? PauseDate,
            float Probability, int ProductTypeId, int Runs, DateTime StartDate, long StationId, string Status, int SuccessfulRuns);
    }
}
