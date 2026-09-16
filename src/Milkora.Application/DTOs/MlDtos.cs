namespace Milkora.Application.DTOs;

public class ForecastPointDto
{
    public int Step { get; set; }
    public string Date { get; set; } = string.Empty;   // yyyy-MM-dd
    public double Value { get; set; }
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }
}

public class MilkForecastDto
{
    public int Days { get; set; }
    public double ProjectedTotal { get; set; }
    public double DailyAverage { get; set; }
    public string Basis { get; set; } = string.Empty;     // "ssa" | "average"
    public string? Message { get; set; }
    public List<ForecastPointDto> Points { get; set; } = new();
}

public class MilkAnomalyDto
{
    public Guid? AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public string Date { get; set; } = string.Empty;
    public double Value { get; set; }
    public double Expected { get; set; }
    public double DropPercent { get; set; }
    public string Severity { get; set; } = "Low";         // Low | Medium | High
}

public class AnomalyReportDto
{
    public int AnimalsAnalysed { get; set; }
    public string? Message { get; set; }
    public List<MilkAnomalyDto> Anomalies { get; set; } = new();
}

public class MilkHealthDto
{
    public Guid AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public string Status { get; set; } = "Unknown";     // Healthy | Watch | AtRisk | Unknown
    public int Score { get; set; }
    public List<string> Reasons { get; set; } = new();
    public double BaselineAvg { get; set; }
    public double RecentAvg { get; set; }
    public double TrendPercent { get; set; }
    public double DropPercent { get; set; }
}
