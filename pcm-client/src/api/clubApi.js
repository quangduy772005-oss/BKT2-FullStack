import axios from "./axios";

export const getClubs = () => axios.get("/clubs");
export const createClub = (data) => axios.post("/clubs", data);
export const deleteClub = (id) => axios.delete(`/clubs/${id}`);
