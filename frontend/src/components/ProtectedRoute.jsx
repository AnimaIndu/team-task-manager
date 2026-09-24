import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

// roles is optional, e.g. roles={['Admin', 'Manager']}
export default function ProtectedRoute({ children, roles }) {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  if (roles && !roles.includes(user.role)) return <Navigate to="/" replace />;
  return children;
}
