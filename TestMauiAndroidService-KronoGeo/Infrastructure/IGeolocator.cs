using System;
using System.Collections.Generic;
using System.Text;

namespace TestMauiAndroidService_KronoGeo.Infrastructure
{
    public interface IGeolocator
    {
        Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken);
    }
}
