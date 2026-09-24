import { useEffect, useState } from 'react';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';
import { Link } from 'react-router-dom';

export default function Dashboard() {
  const { user, logout } = useAuth();
  const [data, setData] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/dashboard')
      .then((res) => setData(res.data))
      .catch(() => setError('Could not load dashboard'));
  }, []);

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: 24 }}>
      
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}
      >
        <h1>Dashboard</h1>

        <div>
          {user.name} ({user.role}){' '}
          <Link to="/tasks">View Tasks</Link>{' '}
          <Link to="/notifications">Notifications</Link>{' '}
          <button onClick={logout}>Log out</button>
        </div>
      </div>

      {error && <p className="error">{error}</p>}

      {!data && !error && <p>Loading...</p>}

      {data && (
        <>
          <div className="card">
            <h3>Overdue tasks: {data.overdueCount}</h3>
          </div>

          <div className="card">
            <h3>By status</h3>

            {data.statusCounts.length === 0 && (
              <p>No tasks yet.</p>
            )}

            {data.statusCounts.map((s) => (
              <p key={s.status}>
                {s.status}: {s.count}
              </p>
            ))}
          </div>

          <div className="card">
            <h3>By priority</h3>

            {data.priorityCounts.length === 0 && (
              <p>No tasks yet.</p>
            )}

            {data.priorityCounts.map((p) => (
              <p key={p.priority}>
                {p.priority}: {p.count}
              </p>
            ))}
          </div>
        </>
      )}
    </div>
  );
}