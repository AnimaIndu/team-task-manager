import { useEffect, useState } from 'react';
import api from '../api/axios';

export default function Notifications() {
  const [notifications, setNotifications] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/notifications')
      .then((res) => {
        setNotifications(res.data);
      })
      .catch((err) => {
        console.error('Load notifications error:', err);
        setError('Could not load notifications');
      });
  }, []);

  const markAsRead = (id) => {
    api.patch(`/notifications/${id}/read`)
      .then(() => {
        setNotifications((prev) =>
          prev.map((n) =>
            n.id === id ? { ...n, read: true } : n
          )
        );
      })
      .catch((err) => {
        console.error('Mark notification as read error:', err);
      });
  };

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: 24 }}>
      <h1>Notifications</h1>

      {error && <p className="error">{error}</p>}

      {notifications.length === 0 && !error && (
        <p>No notifications yet.</p>
      )}

      {notifications.map((notification) => (
        <div
          className="card"
          key={notification.id}
          style={{
            marginBottom: 10,
            padding: 12,
            opacity: notification.read ? 0.6 : 1
          }}
        >
          <p>{notification.message}</p>

          <small>
            {new Date(notification.createdAt).toLocaleString()}
          </small>

          {!notification.read && (
            <div>
              <button
                onClick={() => markAsRead(notification.id)}
                style={{ marginTop: 8 }}
              >
                Mark as Read
              </button>
            </div>
          )}
        </div>
      ))}
    </div>
  );
}