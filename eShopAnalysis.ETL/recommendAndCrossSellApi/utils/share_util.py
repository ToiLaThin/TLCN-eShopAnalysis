from pyspark import SparkConf
from pyspark.sql import SparkSession
import os 
import findspark
def get_root_path():
    ROOT_DIR = os.path.abspath(os.curdir)
    return ROOT_DIR

def init_spark_session():
    findspark.init('D:\Hadoop_Ecosystem\spark-3.5.0-bin-hadoop3')

    master = "local[*]"
    config = (
        SparkConf()
        .setAppName("Transform")
        .setMaster(master)
        .set("spark.hadoop.hive.metastore.uris", "thrift://localhost:9083")
        .set("spark.sql.warehouse.dir", "hdfs://localhost:9000/ducthinh/hive/warehouse")
        .set("spark.memory.offHeap.enabled", "true")
        .set("spark.memory.offHeap.size", "10g")
        .set("spark.executor.memory", "4g")
        .set("hive.exec.dynamic.partition", "true")
        .set("hive.exec.dynamic.partition.mode", "nonstrict")
        .set("spark.sql.session.timeZone", "UTC+7")
        .set("spark.network.timeout", "50000")
        .set("spark.executor.heartbeatInterval", "5000")
        .set("spark.worker.timeout", "5000")
    )

    ss = (
        SparkSession.builder.config(conf=config)
        .enableHiveSupport()
        .getOrCreate()
    )

    ss.sparkContext.setLogLevel("ERROR")
    return ss