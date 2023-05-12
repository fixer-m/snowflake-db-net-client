using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Snowflake.Client.ObjectPool
{
    /// <summary>
    /// A policy for pooling <see cref="StringBuilder"/> instances.
    /// </summary>
    internal class CustomStringBuilderPooledObjectPolicy : PooledObjectPolicy<StringBuilder>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled <see cref="StringBuilder"/> instances.
        /// </summary>
        /// <value>Defaults to <c>100</c>.</value>
        public int InitialCapacity { get; set; } = 1024;

        /// <summary>
        /// Gets or sets the maximum value for <see cref="StringBuilder.Capacity"/> that is allowed to be
        /// retained, when <see cref="Return(StringBuilder)"/> is invoked.
        /// </summary>
        /// <value>Defaults to <c>4096</c>.</value>
        public int MaximumRetainedCapacity { get; set; } = 16 * 1024;

        /// <inheritdoc />
        public override StringBuilder Create()
        {
            return new StringBuilder(InitialCapacity);
        }

        /// <inheritdoc />
        public override bool Return(StringBuilder obj)
        {
            if (obj.Capacity > MaximumRetainedCapacity)
            {
                // Too big. Discard this one.
                return false;
            }

            obj.Clear();
            return true;
        }
    }
}
