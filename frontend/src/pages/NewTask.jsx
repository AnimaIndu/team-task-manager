
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios';

export default function NewTask() {
  const navigate = useNavigate();

  const [teams, setTeams] = useState([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState('Medium');
  const [assignedToId, setAssignedToId] = useState('');
  const [teamId, setTeamId] = useState('');
  const [deadline, setDeadline] = useState('');
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // Load teams
  useEffect(() => {
    api.get('/teams')
      .then((res) => {
        setTeams(res.data);
      })
      .catch((err) => {
        console.error('Load teams error:', err);
        console.error('Response data:', err.response?.data);

        setError(
          err.response?.data?.message ||
          JSON.stringify(err.response?.data) ||
          `Could not load teams (${err.response?.status || 'unknown error'})`
        );
      });
  }, []);

  // Create task
  const handleSubmit = (e) => {
    e.preventDefault();

    setError('');
    setSubmitting(true);

    const priorityValue =
      priority === 'Low'
        ? 0
        : priority === 'Medium'
          ? 1
          : 2;

    const taskData = {
      title,
      description,
      priority: priorityValue,
      assignedToId: Number(assignedToId),
      teamId: teamId ? Number(teamId) : null,
      deadline: deadline || null,
    };

    console.log('Creating task with:', taskData);

    api.post('/tasks', taskData)
      .then((res) => {
        console.log('Task created successfully:', res.data);
        navigate(`/tasks/${res.data.id}`);
      })
      .catch((err) => {
        console.error('Create task error:', err);
        console.error('Response status:', err.response?.status);
        console.error('Response data:', err.response?.data);

        setError(
          err.response?.data?.message ||
          JSON.stringify(err.response?.data) ||
          `Could not create task (${err.response?.status || 'unknown error'})`
        );
      })
      .finally(() => {
        setSubmitting(false);
      });
  };

  return (
    <div style={{ maxWidth: 600, margin: '0 auto', padding: 24 }}>
      <h1>New Task</h1>

      {error && <p className="error">{error}</p>}

      <form onSubmit={handleSubmit}>
        {/* Title */}
        <div>
          <input
            type="text"
            placeholder="Title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>

        {/* Description */}
        <div>
          <textarea
            placeholder="Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>

        {/* Priority */}
        <div>
          <select
            value={priority}
            onChange={(e) => setPriority(e.target.value)}
          >
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
          </select>
        </div>

        {/* Team */}
        <div>
          <select
            value={teamId}
            onChange={(e) => setTeamId(e.target.value)}
            required
          >
            <option value="">Select team</option>

            {teams.map((t) => (
              <option key={t.id} value={t.id}>
                {t.name}
              </option>
            ))}
          </select>
        </div>

        {/* Assigned User */}
        <div>
          <input
            type="number"
            placeholder="Assign to User ID"
            value={assignedToId}
            onChange={(e) => setAssignedToId(e.target.value)}
            required
          />
        </div>

        {/* Deadline */}
        <div>
          <input
            type="date"
            value={deadline}
            onChange={(e) => setDeadline(e.target.value)}
          />
        </div>

        {/* Submit */}
        <button type="submit" disabled={submitting}>
          {submitting ? 'Creating...' : 'Create Task'}
        </button>
      </form>
    </div>
  );
}
