using System;

namespace PlatoCore.Messaging.Abstractions
{

    public interface IBrokerSubscriber 
    {

        void Subscribe();

        void Unsubscribe();

    }

}
