export interface AuthResponse {
    email : string;
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
    tokenType: string;
}

