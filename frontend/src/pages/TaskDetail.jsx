import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import api from '../api/axios';

export default function TaskDetail() {
  const { id } = useParams();
  const [task, setTask] = useState(null);
  const [error, setError] = useState('');
  const [commentText, setCommentText] = useState('');
  const [posting, setPosting] = useState(false);

  useEffect(() => {
    api.get(`/tasks/${id}`)
      .then((res) => setTask(res.data))
      .catch(() => setError('Could not load task'));
  }, [id]);

  const handleAddComment = (e) => {
    e.preventDefault();
    if (!commentText.trim()) return;
    setPosting(true);
    api.post(`/tasks/${id}/comments`, { text: commentText })
      .then((res) => {
        setTask((prev) => ({ ...prev, comments: [...(prev.comments || []), res.data] }));
        setCommentText('');
      })
      .catch(() => setError('Could not post comment'))
      .finally(() => setPosting(false));
  };

//   const handleStatusChange = (newStatus) => {
//     api.put(`/tasks/${id}`, { status: newStatus })
//       .then((res) => setTask(res.data))
//       .catch(() => setError('Could not update status'));
//   };

const handleStatusChange = (newStatus) => {
  setError('');

  console.log('Updating status to:', newStatus);

  api.put(`/tasks/${id}`, { status: newStatus })
    .then((res) => {
      console.log('Status updated successfully:', res.data);
      setTask(res.data);
    })
    .catch((err) => {
      console.error('Update status error:', err);
      console.error('Response status:', err.response?.status);
      console.error('Response data:', err.response?.data);

      setError(
        err.response?.data?.message ||
        JSON.stringify(err.response?.data) ||
        `Could not update status (${err.response?.status || 'unknown error'})`
      );
    });
};
  if (error) return <p className="error">{error}</p>;
  if (!task) return <p>Loading...</p>;

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: 24 }}>
      <h1>{task.title}</h1>
      <p>{task.description}</p>
      <p>Status: {task.status} | Priority: {task.priority}</p>

      {/* NEW — status buttons */}
      <div style={{ display: 'flex', gap: 8, margin: '8px 0' }}>
        {/* <button disabled={task.status === 'ToDo'} onClick={() => handleStatusChange('ToDo')}>ToDo</button>
        <button disabled={task.status === 'InProgress'} onClick={() => handleStatusChange('InProgress')}>In Progress</button>
        <button disabled={task.status === 'Done'} onClick={() => handleStatusChange('Done')}>Done</button> */}

        <button disabled={task.status === 'ToDo'} onClick={() => handleStatusChange(0)}>ToDo</button>

<button disabled={task.status === 'InProgress'} onClick={() => handleStatusChange(1)}>
  In Progress
</button>

<button disabled={task.status === 'Done'} onClick={() => handleStatusChange(2)}>
  Done
</button>
      </div>

      <p>Assigned to: {task.assignedToName ?? task.assignedTo?.name}</p>
      {task.deadline && <p>Deadline: {new Date(task.deadline).toLocaleDateString()}</p>}

      <h3>Comments</h3>
      {task.comments && task.comments.length === 0 && <p>No comments yet.</p>}
      {task.comments && task.comments.map((c) => (
        <div className="card" key={c.id}>
          <p>{c.text}</p>
          <small>{c.authorName ?? c.author?.name} — {new Date(c.createdAt).toLocaleString()}</small>
        </div>
      ))}

      <form onSubmit={handleAddComment} style={{ marginTop: 12 }}>
        <input
          type="text"
          value={commentText}
          onChange={(e) => setCommentText(e.target.value)}
          placeholder="Add a comment..."
          style={{ width: '100%', padding: 8 }}
        />
        <button type="submit" disabled={posting} style={{ marginTop: 8 }}>
          {posting ? 'Posting...' : 'Post Comment'}
        </button>
      </form>
    </div>
  );
}