using PlatoCore.Models.Users;

namespace PlatoCore.Models.Notifications
{
    
    public interface INotification
    {
        
        IUser To { get; set; }

        IUser From { get; set; }
        
        INotificationType Type { get; }
        
    }

}
