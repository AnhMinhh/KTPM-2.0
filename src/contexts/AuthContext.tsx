import React, { createContext, useContext, useState, useEffect } from 'react';
import { useToast } from '@/hooks/use-toast';
import { api } from '@/lib/api';

const AUTH_TOKEN_STORAGE_KEY = 'auth_token';

interface User {
  id: string;
  email: string | null;
}

interface Profile {
  id: string;
  user_id: string;
  email: string;
  username: string;
  full_name: string | null;
  avatar_url: string | null;
  phone: string | null;
}

interface AuthContextType {
  user: User | null;
  profile: Profile | null;
  isAdmin: boolean;
  isLoading: boolean;
  signUp: (email: string, password: string, username: string, fullName: string) => Promise<{ error: Error | null }>;
  signIn: (email: string, password: string) => Promise<{ error: Error | null }>;
  signOut: () => Promise<void>;
  updateProfile: (updates: Partial<Profile>) => Promise<{ error: Error | null }>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [profile, setProfile] = useState<Profile | null>(null);
  const [isAdmin, setIsAdmin] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const { toast } = useToast();

  useEffect(() => {
    const init = async () => {
      let token: string | null = null;
      try {
        token = localStorage.getItem(AUTH_TOKEN_STORAGE_KEY);
      } catch {
        token = null;
      }

      if (!token) {
        setIsLoading(false);
        return;
      }

      const { data, error } = await api.get<{ user: User; profile: Profile | null; roles: string[] }>(
        '/me'
      );

      if (error || !data) {
        try {
          localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY);
        } catch {
          // ignore
        }
        setUser(null);
        setProfile(null);
        setIsAdmin(false);
        setIsLoading(false);
        return;
      }

      setUser(data.user);
      setProfile(data.profile);
      setIsAdmin((data.roles || []).includes('admin'));
      setIsLoading(false);
    };

    init();
  }, []);

  interface AuthResponse {
    token: string;
    user: User;
    profile: Profile | null;
    roles: string[];
  }

  const signUp = async (email: string, password: string, username: string, fullName: string) => {
    try {
      const { data, error } = await api.post<AuthResponse>('/auth/signup', {
        email,
        password,
        username,
        full_name: fullName,
      });

      if (error || !data) {
        return { error: new Error(error || 'Signup failed') };
      }

      try {
        localStorage.setItem(AUTH_TOKEN_STORAGE_KEY, data.token);
      } catch {
        // ignore
      }

      setUser(data.user);
      setProfile(data.profile);
      setIsAdmin((data.roles || []).includes('admin'));

      toast({
        title: 'Account created!',
        description: 'Welcome to ElectroHub!',
      });

      return { error: null };
    } catch (error) {
      return { error: error as Error };
    }
  };

  const signIn = async (email: string, password: string) => {
    try {
      const { data, error } = await api.post<AuthResponse>('/auth/signin', {
        email,
        password,
      });

      if (error || !data) {
        return { error: new Error(error || 'Login failed') };
      }

      try {
        localStorage.setItem(AUTH_TOKEN_STORAGE_KEY, data.token);
      } catch {
        // ignore
      }

      setUser(data.user);
      setProfile(data.profile);
      setIsAdmin((data.roles || []).includes('admin'));

      toast({
        title: 'Welcome back!',
        description: 'Successfully logged in.',
      });

      return { error: null };
    } catch (error) {
      return { error: error as Error };
    }
  };

  const signOut = async () => {
    try {
      localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY);
    } catch {
      // ignore
    }
    setUser(null);
    setProfile(null);
    setIsAdmin(false);
    
    toast({
      title: 'Logged out',
      description: 'See you soon!',
    });
  };

  const updateProfile = async (updates: Partial<Profile>) => {
    if (!user) return { error: new Error('Not authenticated') };

    try {
      const { data, error } = await api.put<{ profile: Profile }>('/me/profile', updates);

      if (error || !data) {
        throw new Error(error || 'Failed to update profile');
      }

      setProfile(data.profile);

      toast({
        title: 'Profile updated',
        description: 'Your changes have been saved.',
      });

      return { error: null };
    } catch (error) {
      return { error: error as Error };
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        profile,
        isAdmin,
        isLoading,
        signUp,
        signIn,
        signOut,
        updateProfile,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
