using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aatrox.ShadowHunters.Interfaces
{
    /// <summary>
    ///     Represents a character card.
    /// </summary>
    public interface IPlayerCard : ICard
    {
        /// <summary>
        ///     Special spell along with the player character.
        /// </summary>
        ICharacterSpell ActiveSpell { get; }

        /// <summary>
        ///     Faction of the character.
        /// </summary>
        PlayerFaction Faction { get; }

        /// <summary>
        ///     Maximum life of the character
        /// </summary>
        int Life { get; }
    }

    /// <summary>
    ///     Represents a player.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        ///     Color of the player. (dice color)
        /// </summary>
        PlayerColor Color { get; }

        /// <summary>
        ///     Player character card.
        /// </summary>
        IPlayerCard PlayerCard { get; }

        /// <summary>
        ///     Inventory of the player.
        /// </summary>
        IReadOnlyList<ICard> Inventory { get; }

        /// <summary>
        ///     Amount of damage taken.
        /// </summary>
        int DamagesTaken { get; }

        /// <summary>
        ///     Remaining life points.
        /// </summary>
        int CurrentLife => PlayerCard.Life - DamagesTaken;

        /// <summary>
        ///     Whether the player is alive or not.
        /// </summary>
        bool IsAlive => CurrentLife <= 0;

        /// <summary>
        ///     Whether this player has revealed itself or not.
        /// </summary>
        bool IsRevealed { get; }
    }

    /// <summary>
    ///     Represents the game itself.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        ///     Players in the current game.
        /// </summary>
        IReadOnlyList<IPlayer> Players { get; }

        /// <summary>
        ///     Stack of the current available vision cards.
        /// </summary>
        Stack<ICard> VisionCards { get; }

        /// <summary>
        ///     Stack of the current available light cards.
        /// </summary>
        Stack<ICard> LightCards { get; }

        /// <summary>
        ///     Stack of the current available dark cards.
        /// </summary>
        Stack<ICard> DarkCards { get; }

        /// <summary>
        ///     List of every discarded and/or used cards.
        /// </summary>
        List<ICard> Discard { get; }

        /// <summary>
        ///     Gets the three zones with their two places.
        /// </summary>
        List<(IPlace, IPlace)> Zones { get; }

        /// <summary>
        ///     Starts the game.
        /// </summary>
        Task StartAsync();

        /// <summary>
        ///     Ends the game.
        /// </summary>
        Task StopAsync();
    }

    /// <summary>
    ///     Represent a place in its zone.
    /// </summary>
    public interface IPlace : IActiveCard
    {
        /// <summary>
        ///     Gets the neighbor of the current place card. This can be
        ///     usefull when determining the three different zones.
        /// </summary>
        IPlace Neighbor { get; }
    }

    /// <summary>
    ///     Represent a spell of a character.
    /// </summary>
    public interface ICharacterSpell : IPassiveCard, IActiveCard
    {
        /// <summary>
        ///     Owner of the spell.
        /// </summary>
        IPlayer Owner { get; }

        /// <summary>
        ///     Determines if <see cref="IPassiveCard.ApplyAsync"/> can be used.
        /// </summary>
        bool HasPassive { get; }

        /// <summary>
        ///     Indicates whether the spell can be reused or not.
        /// </summary>
        bool CanReuse { get; }

        /// <summary>
        ///     Indicates if the spell was used.
        /// </summary>
        bool IsUsed { get; }
    }

    /// <summary>
    ///     Represents a card with a passive.
    /// </summary>
    /// <remarks>
    ///     Exemple: "Gives one more damage"
    /// </remarks>
    public interface IPassiveCard : ICard
    {
        /// <summary>
        ///     Apply an effect of an equipment.
        /// </summary>
        /// <remarks>
        ///     For exemple, if you have an equipment that gives you one damage,
        ///     damage calculation will be updated and your damage count will
        ///     be increased by one. This mechanism applies for every kind of 
        ///     equipment.
        /// </remarks>
        Task ApplyAsync();
    }

    /// <summary>
    ///     Represents a card with an active.
    /// </summary>
    /// <remarks>
    ///     Exemple: Damage 2 points to the player of your choice and
    ///     heal youself 2 points of damage.
    /// </remarks>
    public interface IActiveCard : ICard
    {
        /// <summary>
        ///     Uses the active of a card.
        /// </summary>
        Task UseAsync();
    }

    /// <summary>
    ///     Represent a card of the game.
    /// </summary>
    public interface ICard
    {
        /// <summary>
        ///     Unique id of the card. This is used for deserialization of the 
        ///     different cards, to determine their specific spell, or effect.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Type of the card.
        /// </summary>
        CardType Type { get; }

        /// <summary>
        ///     Resets the different attributes of the card. Useful when it has
        ///     been used and goes into the discard.
        /// </summary>
        Task ResetAsync();
    }

    /// <summary>
    ///     Enumerates the different type of cards.
    /// </summary>
    public enum CardType
    {
        Player,
        Place,
        Vision,
        Light,
        Dark,
    }

    /// <summary>
    ///     Enumerates the different player colours.
    /// </summary>
    public enum PlayerColor
    {
        Black,
        White,
        Yellow,
        Green,
        Blue,
        Orange,
        Violet,
        Red
    }

    /// <summary>
    ///     Enumerates the different player factions.
    /// </summary>
    public enum PlayerFaction
    {
        Shadow,
        Hunter,
        Neutral
    }
}
