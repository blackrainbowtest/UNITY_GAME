/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestEvent.cs                                         /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 12:45:07 by UDA                                      */
/*   Updated: 2025/12/01 12:45:07 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

public enum RestEventType
{
    None,
    MinorAmbush,    // лёгкое нападение
    MajorAmbush,    // серьёзное нападение → бой
    Noise,          // шум → отдых продолжается с штрафом
    WeatherChange   // ухудшение погоды
}

public class RestEvent
{
    public RestEventType type;
    public float penaltyMultiplier; // например, 0.8f (80% эффективности отдыха)
}
