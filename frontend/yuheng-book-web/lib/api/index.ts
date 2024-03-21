import axios from 'axios';
import { Books } from './books';

export const myAxios = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  timeout: 1000,
});

export const Api = {
  Books,
};
