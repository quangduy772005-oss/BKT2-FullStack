import api from "./api";

export interface Member {
  id?: number;
  fullName: string;
  email: string;
  joinDate: string;
  clubId: number;
}

// ✅ User + Admin
export const getMembers = () => {
  return api.get("/MembersApi");
};

// ❌ Chỉ Admin
export const createMember = (member: Member) => {
  return api.post("/MembersApi", member);
};
