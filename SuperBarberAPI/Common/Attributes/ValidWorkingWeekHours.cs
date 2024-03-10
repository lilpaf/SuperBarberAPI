using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Property)]
public class ValidWorkingWeekHours : ValidationAttribute
{
    private readonly Regex _regex;

    public ValidWorkingWeekHours()
    {
        _regex = new(@"\b(?:0[0-9]|1[0-9]|2[0-3]):(?:00|30)\b");
    }

    public override bool IsValid(object? value)
    {
        if (value is not Dictionary<string, Tuple<string?, string?>> workingWeekHours)
        {
            return false;
        }

        if (workingWeekHours.Keys.Any(day => !Enum.GetNames<DayOfWeek>().Contains(day)))
        {
            return false;
        }

        foreach (var hours in workingWeekHours.Values)
        {
            string? startHour = hours.Item1;
            string? finishHour = hours.Item2;

            if (startHour is null && finishHour is null)
            {
                continue;
            }
            else if (startHour is null || finishHour is null)
            {
                return false;
            }

            if (!_regex.IsMatch(startHour) || !_regex.IsMatch(finishHour))
            {
                return false;
            }
        }

        return true;
    }
}
