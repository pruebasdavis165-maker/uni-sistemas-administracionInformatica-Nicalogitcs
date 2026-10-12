"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import * as authService from "@/services/api/authService";
import { TOKEN_KEY } from "@/services/api/client";
import { usuarioFromToken } from "@/lib/jwt";
import type { LoginRequest, RegisterRequest, Usuario } from "@/types";

const USER_KEY = "nicalogistics_user";

interface AuthContextValue {
  user: Usuario | null;
  token: string | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function readStoredUser(): Usuario | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as Usuario;
  } catch {
    return null;
  }
}

function persistSession(token: string, user: Usuario) {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<Usuario | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem(TOKEN_KEY);
    if (!storedToken) {
      setIsLoading(false);
      return;
    }
    const fromToken = usuarioFromToken(storedToken);
    const storedUser = readStoredUser();
    setToken(storedToken);
    setUser(fromToken ?? storedUser);
    setIsLoading(false);
  }, []);

  const applyAuth = useCallback((newToken: string) => {
    const parsed = usuarioFromToken(newToken);
    if (!parsed) {
      throw new Error("No se pudo leer la información del token.");
    }
    persistSession(newToken, parsed);
    setToken(newToken);
    setUser(parsed);
  }, []);

  const login = useCallback(
    async (request: LoginRequest) => {
      const { token: newToken } = await authService.login(request);
      applyAuth(newToken);
    },
    [applyAuth],
  );

  const register = useCallback(
    async (request: RegisterRequest) => {
      const { token: newToken } = await authService.register(request);
      applyAuth(newToken);
    },
    [applyAuth],
  );

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isLoading,
      isAuthenticated: Boolean(token && user),
      isAdmin: user?.rol === "Administrador",
      login,
      register,
      logout,
    }),
    [user, token, isLoading, login, register, logout],
  );

  return (
    <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth debe usarse dentro de AuthProvider");
  }
  return ctx;
}
