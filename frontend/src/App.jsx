import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Tasks from './pages/Tasks';
import TaskDetail from './pages/TaskDetail';
import NewTask from './pages/NewTask';
import Notifications from './pages/Notifications';


export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
      <Route path="*" element={<Navigate to="/" replace />} />
      <Route path="/tasks" element={<ProtectedRoute><Tasks /></ProtectedRoute>} />
      <Route path="/tasks/:id" element={<ProtectedRoute><TaskDetail /></ProtectedRoute>} />
      <Route path="/tasks/new" element={<ProtectedRoute><NewTask /></ProtectedRoute>} />
      <Route path="/tasks/new" element={<ProtectedRoute><NewTask /></ProtectedRoute>} />
      <Route path="/notifications" element={<Notifications />} />
      
    </Routes>
  );
}
