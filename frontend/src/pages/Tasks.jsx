import { useEffect, useState } from 'react';
import api from '../api/axios';
import { Link } from 'react-router-dom';

export default function Tasks() {
  const [tasks, setTasks] = useState(null);
  const [error, setError] = useState('');
  const [status, setStatus] = useState('');
  const [priority, setPriority] = useState('');

  useEffect(() => {
    setTasks(null);
    api.get('/tasks', { params: { status: status || undefined, priority: priority || undefined } })
      .then((res) => setTasks(res.data))
      .catch(() => setError('Could not load tasks'));
  }, [status, priority]);

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: 24 }}>
      <h1>Tasks</h1>

      <div style={{ marginBottom: 16 }}>
        <Link to="/tasks/new">+ New Task</Link>
      </div>

      <div style={{ marginBottom: 16, display: 'flex', gap: 12 }}>
        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="">All statuses</option>
          <option value="ToDo">ToDo</option>
          <option value="InProgress">In Progress</option>
          <option value="Done">Done</option>
        </select>

        <select value={priority} onChange={(e) => setPriority(e.target.value)}>
          <option value="">All priorities</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>
      </div>

      {error && <p className="error">{error}</p>}
      {!tasks && !error && <p>Loading...</p>}
      {tasks && tasks.length === 0 && <p>No tasks found.</p>}

      {tasks && tasks.map((t) => (
        <div className="card" key={t.id}>
          <h3><Link to={`/tasks/${t.id}`}>{t.title}</Link></h3>
          <p>Status: {t.status} | Priority: {t.priority}</p>
        </div>
      ))}
    </div>
  );
}