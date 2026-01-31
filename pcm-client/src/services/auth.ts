import api from "./api";

interface LoginResponse {
  accessToken: string;
  refreshToken?: string;
  user?: {
    id: string;
    email: string;
    fullName?: string;
  };
}

export const login = async (email: string, password: string): Promise<LoginResponse> => {
  const res = await api.post<LoginResponse>("/auth/login", {
    email,
    password,
  });

  if (res.data?.accessToken) {
    localStorage.setItem("token", res.data.accessToken);
    if (res.data.user) {
      localStorage.setItem("user", JSON.stringify(res.data.user));
    }
  }

  return res.data;
};

export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("user");
};

export const getToken = () => {
  return localStorage.getItem("token");
};

export const getUser = () => {
  const user = localStorage.getItem("user");
  return user ? JSON.parse(user) : null;
};

export const isAuthenticated = () => {
  return !!getToken();
};

export const register = async (email: string, password: string, fullName?: string) => {
  const res = await api.post("/auth/register", { 
    email, 
    password,
    confirmPassword: password,
    fullName 
  });
  return res.data;
};

