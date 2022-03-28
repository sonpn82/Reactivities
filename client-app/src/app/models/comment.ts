// same fields with commentDto in backend
export interface ChatComment {
  id: number;
  createdAt: Date;
  body: string;
  username: string;
  displayName: string;
  image: string;
}