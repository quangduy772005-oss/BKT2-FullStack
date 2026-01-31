import axios from "./axios";

export const getActivities = () => axios.get("/activities");
export const createActivity = (data) => axios.post("/activities", data);
export const deleteActivity = (id) => axios.delete(`/activities/${id}`);
