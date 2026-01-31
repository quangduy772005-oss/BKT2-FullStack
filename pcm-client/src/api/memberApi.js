import axios from "./axios";

export const getMembers = () => {
  return axios.get("/members");
};

export const getMemberById = (id) => {
  return axios.get(`/members/${id}`);
};

export const createMember = (data) => {
  return axios.post("/members", data);
};

export const updateMember = (id, data) => {
  return axios.put(`/members/${id}`, data);
};

export const deleteMember = (id) => {
  return axios.delete(`/members/${id}`);
};
