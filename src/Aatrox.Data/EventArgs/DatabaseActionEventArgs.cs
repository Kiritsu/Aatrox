using System;
using Aatrox.Data.Enums;

namespace Aatrox.Data.EventArgs
{
    /// <summary>
    ///     EventArgs used in a <see cref="Func{T, TResult}"/> which is fired whenever an action is made in the <see cref="AatroxDbContext"/>.
    /// </summary>
    public sealed class DatabaseActionEventArgs
    {
        /// <summary>
        ///     Type of the action made in the Database.
        /// </summary>
        public ActionType Type { get; set; }

        /// <summary>
        ///     Entity that is subject of that context. Can be null when <see cref="Type"/> is Save.
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        ///     Type of the <see cref="Entity"/>.
        /// </summary>
        public Type EntityType => Entity.GetType();

        /// <summary>
        ///     Path of the action: ::/Repository/ActionType/[Entity]
        /// </summary>
        /// <remarks>
        ///     '[]' mean optional.
        /// </remarks>
        public string Path { get; set; }

        /// <summary>
        ///     Indicates that <see cref="Exception"/> is not null.
        /// </summary>
        public bool IsErrored { get; set; }

        /// <summary>
        ///     Exception thrown in/by the Database.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
