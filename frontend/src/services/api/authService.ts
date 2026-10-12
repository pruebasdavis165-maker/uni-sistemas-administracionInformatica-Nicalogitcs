import type { LoginRequest, LoginResponse, RegisterRequest } from "@/types";
import { apiClient } from "./client";

export async function login(request: LoginRequest): Promise<LoginResponse> {
  return apiClient<LoginResponse>("/Auth/login", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export async function register(
  request: RegisterRequest,
): Promise<LoginResponse> {
  return apiClient<LoginResponse>("/Auth/register", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
