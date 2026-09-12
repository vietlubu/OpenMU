// <copyright file="CollectionAdapterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Specialized;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests the <see cref="CollectionAdapter{TClass, TEfCore}"/>.
/// </summary>
[TestFixture]
public class CollectionAdapterTest
{
    /// <summary>
    /// Ensures clearing a watched collection emits a valid reset event.
    /// </summary>
    [Test]
    public void ClearWithSubscriberEmitsReset()
    {
        var adapter = new CollectionAdapter<object, string>(new List<string> { "item" });
        NotifyCollectionChangedEventArgs? eventArgs = null;
        adapter.CollectionChanged += (_, args) => eventArgs = args;

        adapter.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(adapter, Is.Empty);
            Assert.That(eventArgs?.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
        });
    }
}
