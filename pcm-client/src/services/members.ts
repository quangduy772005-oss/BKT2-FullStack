import api from "./api";

export interface Member {
  id: number;
  fullName: string;
  email: string;
}

export const getMembers = async () => {
  return api.get<Member[]>("/members");
};
